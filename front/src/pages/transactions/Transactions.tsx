import { useEffect, useState } from "react";
import GenericTable, { type Column } from "../../components/GenericTable";
import Modal from "../../components/Modal";
import IconPencil from "../../icons/IconPencil";
import IconTrash from "../../icons/IconTrash";
import { useSearchParams } from "react-router-dom";
import { IPaginationResult } from "@/shared/interfaces/pagination.interface";
import { Formik } from "formik";
import { validationSchema } from "./utils";
import { BaseResponse } from "@/shared/services/base.service";
import { toast } from "react-toastify";
import { TransactionService } from "../../modules/transactions/services/transaction.service";
import { CreateOrUpdateTransaction } from "../../modules/transactions/interfaces/create-or-update-transaction";
import { GetTransaction } from "../../modules/transactions/interfaces/get-transaction";
import { AccountService } from "../../modules/accounts/services/accounts.service";
import { GetAccount } from "../../modules/accounts/interfaces/get-account";
import React from "react";

const service = new TransactionService();
const accountService = new AccountService();
const formEmpty: CreateOrUpdateTransaction = {
  type: "credito",
  amount: 0,
  accountId: "",
};

type Props = {
  title?: string;
  report?: boolean;
};

const Transactions: React.FC<Props> = ({
  title = "Movimientos",
  report = false,
}) => {
  const [showForm, setShowModal] = useState(false);
  const [searchParams, setSearchParams] = useSearchParams();
  const [data, setData] = useState<IPaginationResult<GetTransaction> | null>(
    null
  );
  const [isLoading, setIsLoading] = useState(false);
  const [editId, setEditId] = useState<string | null>(null);
  const [initialValues, setInitialValues] =
    useState<CreateOrUpdateTransaction>(formEmpty);
  const [accounts, setAccounts] = useState<GetAccount[]>([]);
  const columns: Column<GetTransaction>[] = [
    { key: "createdAtStr", label: "Fecha" },
    {
      key: "account",
      label: "Cliente",
      render: (_, row) => row.account?.user?.name ?? "N/A",
    },
    {
      key: "account",
      label: "Numero de cuenta",
      render: (_, row) => row.account?.accountNumber ?? "N/A",
    },
    {
      key: "account",
      label: "Tipo de cuenta",
      render: (_, row) => row.account?.accountType ?? "N/A",
    },
    {
      key: "account",
      label: "Saldo inicial",
      render: (_, row) => `$${row.account?.initialBalance}`,
    },
    { key: "status", label: "Estado" },
    { key: "movement", label: "Movimiento" },
    {
      key: "currentBalance",
      label: "Saldo disponible",
      render: (_, row) => `$${row.currentBalance}`,
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

  const getAll = async (q = "", clear = false) => {
    const page = clear ? 1 : searchParams.get("page") ?? 1;
    const query = clear ? "" : q ?? searchParams.get("query") ?? "";
    const from = clear ? "" : searchParams.get("from") ?? "";
    const to = clear ? "" : searchParams.get("to") ?? "";

    try {
      setIsLoading(true);
      const result = await service.search({
        page: Number(page),
        query: query,
        from: from,
        to: to,
        qyt: 10,
        noPaginate: false,
      });

      setData(result.data.data);
    } catch (error) {
      console.log("Error:" + error);
    } finally {
      setIsLoading(false);
    }
  };

  const exportFile = async (type: "pdf" | "json" = "pdf") => {
    const page = searchParams.get("page") ?? 1;
    const from = searchParams.get("from") ?? "";
    const to = searchParams.get("to") ?? "";

    try {
      setIsLoading(true);

      const result = await service.exportFile({
        page: Number(page),
        from: from,
        to: to,
        type: type,
      });

      const link = document.createElement("a");
      link.href = `data:application/${type};base64,${result.data.data}`;
      link.download = `movimientos.${type}`;
      document.body.appendChild(link);
      link.click();
      link.remove();
    } catch (error) {
      console.log("Error:" + error);
    } finally {
      setIsLoading(false);
    }
  };

  const getAccounts = async () => {
    try {
      const { data } = await accountService.getAllPaginated<GetAccount>({
        noPaginate: true,
      });
      setAccounts(data.data.results);
    } catch (error) {
      console.log("Error al obtener usuarios:", error);
    }
  };

  const onSubmit = async (values: CreateOrUpdateTransaction) => {
    try {
      let msg = "Creando con éxito";
      if (editId) {
        await service.edit(editId, values);
        msg = "Actualizado con éxito";
      } else {
        await service.post<CreateOrUpdateTransaction, BaseResponse<string>>(
          values
        );
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

  const handleOnClickFilter = async () => {
    const from = searchParams.get("from") || "";
    const to = searchParams.get("to") || "";

    if (!from || !to) {
      toast("Debe seleccionar ambas fechas", { type: "error" });
      return;
    }

    await getAll();
  };

  const handleChangeSearch = async (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchParams({ query: e.target.value });

    await getAll(e.target.value);
  };

  const handleActionEdit = (data: GetTransaction) => {
    setEditId(data.id);

    setInitialValues({
      ...data,
    });

    setShowModal(true);
  };

  const handleClickDelete = async (id: string) => {
    if (!id) return;

    const result = confirm("¿Está seguro de que desea eliminar este registro?");
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

  const handleClickShow = async () => {
    setShowModal(true);
    setEditId(null);
    setInitialValues(formEmpty);
    await getAccounts();
  };

  const handleOnClickClear = async () => {
    setSearchParams({ page: "1" });
    await getAll("", true);
  };

  const handleClickExport = async (type: "pdf" | "json") => {
    try {
      await exportFile(type);
      toast("Archivo exportado con éxito", { type: "success" });
    } catch (error) {
      console.error("Error al exportar el archivo:", error);
      toast("Error al exportar el archivo", { type: "error" });
    }
  };

  return (
    <section>
      <h2>{title}</h2>
      <div className="section-header">
        {report ? (
          <>
            <input
              type="date"
              value={searchParams.get("from") || ""}
              onChange={(e) => {
                setSearchParams({
                  from: e.target.value,
                  to: searchParams.get("to") || "",
                  page: searchParams.get("page") || "1",
                });
              }}
            />
            <input
              type="date"
              value={searchParams.get("to") || ""}
              onChange={(e) => {
                setSearchParams({
                  from: searchParams.get("from") || "",
                  to: e.target.value,
                  page: searchParams.get("page") || "1",
                });
              }}
            />
            <button onClick={handleOnClickFilter} className="btn">
              Filtrar
            </button>

            {searchParams.get("from") && searchParams.get("to") && (
              <button
                onClick={handleOnClickClear}
                className="btn"
                style={{ background: "red", color: "white" }}
              >
                Limpiar
              </button>
            )}

            <button
              onClick={() => handleClickExport("pdf")}
              className="btn"
              style={{ background: "green", color: "white" }}
            >
              PDF
            </button>

            <button
              onClick={() => handleClickExport("json")}
              className="btn"
              style={{ background: "green", color: "white" }}
            >
              json
            </button>
          </>
        ) : (
          <>
            <input
              type="text"
              onChange={handleChangeSearch}
              placeholder="Buscar"
            />
            <button onClick={handleClickShow} className="btn">
              Nuevo
            </button>
          </>
        )}
      </div>

      <div className="client-list">
        {isLoading ? (
          <div className="loading">Cargando...</div>
        ) : (
          data && (
            <GenericTable
              hideActions={report}
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
        title="Nuevo movimiento"
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
              <label>Cuenta:</label>
              <select
                name="accountId"
                value={values.accountId}
                onChange={handleChange}
              >
                <option value="">Seleccione la cuenta</option>
                {accounts.map((account) => (
                  <option key={account.id} value={account.id}>
                    {account.accountNumber} - {account.user?.name}
                  </option>
                ))}
              </select>
              <span className="text-danger">{errors.accountId}</span>
              <label>Monto:</label>
              <input
                name="amount"
                type="number"
                value={values.amount}
                onChange={handleChange}
                placeholder="Ingrese el monto del movimiento"
              />
              <span className="text-danger">{errors.amount}</span>
              <label>tipo:</label>
              <select name="type" value={values.type} onChange={handleChange}>
                <option value="credito">Credito</option>
                <option value="debito">Debito</option>
              </select>
              <button type="submit" className="btn form-btn">
                Guardar
              </button>
            </form>
          )}
        </Formik>
      </Modal>
    </section>
  );
};

export default Transactions;
