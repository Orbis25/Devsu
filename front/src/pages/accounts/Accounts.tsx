import { useEffect, useState } from "react";
import GenericTable, { type Column } from "../../components/GenericTable";
import Modal from "../../components/Modal";
import IconPencil from "../../icons/IconPencil";
import IconTrash from "../../icons/IconTrash";
import { useSearchParams } from "react-router-dom";
import { IPaginationResult } from "@/shared/interfaces/pagination.interface";
import { GetUser } from "@/modules/users/interfaces/get-user";
import { Formik } from "formik";
import { validationSchema } from "./utils";
import { BaseResponse } from "@/shared/services/base.service";
import { toast } from "react-toastify";
import { AccountService } from "../../modules/accounts/services/accounts.service";
import { CreateOrUpdateAccount } from "../../modules/accounts/interfaces/create-or-update-account";
import { UserService } from "../../modules/users/services/user.service";
import { GetAccount } from "../../modules/accounts/interfaces/get-account";

const service = new AccountService();
const userService = new UserService();

const formEmpty: CreateOrUpdateAccount = {
  accountNumber: "",
  accountType: "ahorros",
  initialBalance: 0,
  userId: "",
};

function Accounts() {
  const [showForm, setShowModal] = useState(false);
  const [searchParams, setSearchParams] = useSearchParams();
  const [data, setData] = useState<IPaginationResult<GetAccount> | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [editId, setEditId] = useState<string | null>(null);
  const [initialValues, setInitialValues] =
    useState<CreateOrUpdateAccount>(formEmpty);
  const [users, setUsers] = useState<GetUser[]>([]);

  const columns: Column<GetAccount>[] = [
    { key: "accountNumber", label: "No. Cuenta" },
    { key: "accountType", label: "Tipo de cuenta" },
    {
      key: "initialBalance",
      label: "Balance inicial",
      render: (_, row) => `$${row.initialBalance}`,
    },
    {
      key: "currentBalance",
      label: "Balance Actual",

      render: (_, row) => `$${row.currentBalance}`,
    },
    {
      key: "dailyDebitLimit",
      label: "Límite diario de débito",
      render: (_, row) => `$${row.dailyDebitLimit}`,
    },
    {
      key: "user",
      label: "Cliente",
      render: (_, row) => row.user?.name ?? "N/A",
    },
    {
      key: "actions",
      label: "Acciones",
      render: (_, row) => (
        <div className="table-actions">
          <button onClick={() => handleActionEdit(row)} className="btn-edit">
            <IconPencil />
          </button>
          <button
            onClick={() => {
              handleClickDelete(row.id);
            }}
            className="btn-delete"
          >
            <IconTrash />
          </button>
        </div>
      ),
    },
  ];

  useEffect(() => {
    (async () => {
      await initialize();
    })();
  }, []);

  const initialize = async () => {
    await getAll();
  };

  const getAll = async (q = "") => {
    const page = searchParams.get("page") ?? 1;
    const query = q ?? searchParams.get("query") ?? "";

    try {
      setIsLoading(true);
      const result = await service.getAllPaginated<GetAccount>({
        page: Number(page),
        query,
      });

      setData(result.data.data);
    } catch (error) {
      console.log("Error:" + error);
    } finally {
      setIsLoading(false);
    }
  };

  const getUsers = async () => {
    try {
      const { data } = await userService.getAllPaginated<GetUser>({
        noPaginate: true,
      });
      setUsers(data.data.results);
    } catch (error) {
      console.log("Error al obtener usuarios:", error);
    }
  };

  const onSubmit = async (values: CreateOrUpdateAccount) => {
    try {
      let msg = "Creando con éxito";
      if (editId) {
        await service.edit(editId, values);
        msg = "Actualizado con éxito";
      } else {
        await service.post<CreateOrUpdateAccount, BaseResponse<string>>(values);
      }

      toast(msg, { type: "success" });

      await getAll();
      setEditId(null);
      setInitialValues(formEmpty);
      setShowModal(false);
    } catch (error) {
      console.log(error);
    }
  };

  const handleChangeSearch = async (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchParams({ query: e.target.value });

    await getAll(e.target.value);
  };

  const handleActionEdit = async (data: GetAccount) => {
    setEditId(data.id);
    await getUsers();

    setInitialValues({
      ...data,
    });

    setShowModal(true);
  };

  const handleClickDelete = async (id: string) => {
    if (!id) return;

    const result = confirm("¿Está seguro de que desea eliminar esta cuenta?");
    if (!result) return;

    try {
      const { data, status } = await service.remove(id);

      if (status != 204 && !data.success) {
        toast(data.message, { type: "error" });
        return;
      }

      toast("Eliminado con éxito", { type: "success" });
      await getAll();
    } catch (error) {
      console.log(error);
    }
  };

  const handleClickNew = async () => {
    await getUsers();
    setEditId(null);
    setInitialValues(formEmpty);
    setShowModal(true);
  };

  return (
    <section>
      <h2>Cuentas</h2>
      <div className="section-header">
        <input type="text" onChange={handleChangeSearch} placeholder="Buscar" />
        <button onClick={handleClickNew} className="btn">
          Nuevo
        </button>
      </div>
      <div className="client-list">
        {isLoading ? (
          <div className="loading">Cargando...</div>
        ) : (
          data && (
            <GenericTable
              columns={columns}
              data={data}
              currentPage={Number(searchParams.get("page")) || 1}
              onChangePage={(page) => {
                setSearchParams({ page: String(page) });
                getAll();
              }}
            />
          )
        )}
      </div>
      <Modal
        open={showForm}
        onClose={() => setShowModal(false)}
        title="Nueva cuenta"
        description="Ingrese los datos"
      >
        <Formik
          onSubmit={onSubmit}
          initialValues={initialValues}
          validationSchema={validationSchema}
          enableReinitialize
        >
          {({ values, errors, handleChange, handleSubmit }) => (
            <form className="form" onSubmit={handleSubmit}>
              <label>No. de cuenta:</label>
              <input
                name="accountNumber"
                value={values.accountNumber}
                onChange={handleChange}
                placeholder="Numero de cuenta"
              />
              <span className="text-danger">{errors.accountNumber}</span>
              <label>Tipo de cuenta:</label>
              <select
                name="accountType"
                value={values.accountType}
                onChange={handleChange}
              >
                <option value="ahorros">Ahorros</option>
                <option value="corriente">Corriente</option>
              </select>

              <label>Balance Inicial:</label>
              <input
                name="initialBalance"
                value={values.initialBalance}
                onChange={handleChange}
                type="number"
                placeholder="Balance inicial de la cuenta"
              />
              <span className="text-danger">{errors.initialBalance}</span>

              <label>Cliente:</label>
              <select
                name="userId"
                value={values.userId}
                onChange={handleChange}
              >
                <option value="">Seleccione un cliente</option>
                {users.map((user) => (
                  <option key={user.id} value={user.id}>
                    {user.name}
                  </option>
                ))}
              </select>
              <span className="text-danger">{errors.userId}</span>

              <button type="submit" className="btn form-btn">
                Guardar
              </button>
            </form>
          )}
        </Formik>
      </Modal>
    </section>
  );
}

export default Accounts;
