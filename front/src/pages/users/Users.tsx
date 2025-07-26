import { useEffect, useState } from "react";
import GenericTable, { type Column } from "../../components/GenericTable";
import Modal from "../../components/Modal";
import IconPencil from "../../icons/IconPencil";
import IconTrash from "../../icons/IconTrash";
import { useSearchParams } from "react-router-dom";
import { IPaginationResult } from "@/shared/interfaces/pagination.interface";
import { GetUser } from "@/modules/users/interfaces/get-user";
import { UserService } from "../../modules/users/services/user.service";
import { Formik } from "formik";
import { CreateOrUpdateUser } from "@/modules/users/interfaces/create-or-update-user";
import { validationSchema } from "./utils";
import { BaseResponse } from "@/shared/services/base.service";
import { toast } from "react-toastify";

const userService = new UserService();
const formEmpty: CreateOrUpdateUser = {
  name: "",
  gender: "hombre",
  age: 0,
  identification: "",
  phone: "",
  address: "",
  clientId: "",
  password: "",
};

function Users() {
  const [showForm, setShowModal] = useState(false);
  const [searchParams, setSearchParams] = useSearchParams();
  const [data, setData] = useState<IPaginationResult<GetUser> | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [editId, setEditId] = useState<string | null>(null);
  const [initialValues, setInitialValues] =
    useState<CreateOrUpdateUser>(formEmpty);

  const columns: Column<GetUser>[] = [
    { key: "name", label: "Nombre" },
    { key: "identification", label: "identification" },

    { key: "gender", label: "Genero" },
    { key: "age", label: "Edad" },
    { key: "phone", label: "Telefono" },
    { key: "address", label: "Dirrecion" },
    { key: "clientId", label: "ID Cliente" },

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
      const result = await userService.getAllPaginated<GetUser>({
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

  const onSubmit = async (values: CreateOrUpdateUser) => {
    try {
      let msg = "Creando con éxito";
      if (editId) {
        await userService.edit(editId, values);
        msg = "Actualizado con éxito";
      } else {
        await userService.post<CreateOrUpdateUser, BaseResponse<string>>(
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

  const handleChangeSearch = async (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchParams({ query: e.target.value });

    await getAll(e.target.value);
  };

  const handleActionEdit = (data: GetUser) => {
    setEditId(data.id);

    setInitialValues({
      ...data,
    });

    setShowModal(true);
  };

  const handleClickDelete = async (id: string) => {
    if (!id) return;

    const result = confirm("¿Está seguro de que desea eliminar este cliente?");
    if (!result) return;

    try {
      const { data, status } = await userService.remove(id);

      if (status != 204 && !data.success) {
        toast(data.message, { type: "error" });
        return;
      }

      toast("Cliente eliminado con éxito", { type: "success" });
      await getAll();
    } catch (error) {
      console.log(error);
    }
  };

  return (
    <section>
      <h2>Clientes</h2>
      <div className="section-header">
        <input type="text" onChange={handleChangeSearch} placeholder="Buscar" />
        <button onClick={() => setShowModal(true)} className="btn">
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
        title="Nuevo cliente"
        description="Ingrese los datos del nuevo cliente"
      >
        <Formik
          onSubmit={onSubmit}
          initialValues={initialValues}
          validationSchema={validationSchema}
          enableReinitialize
        >
          {({ values, errors, handleChange, handleSubmit }) => (
            <form className="form" onSubmit={handleSubmit}>
              <label>Nombre:</label>
              <input
                name="name"
                value={values.name}
                onChange={handleChange}
                placeholder="Nombre cliente"
              />
              <span className="text-danger">{errors.name}</span>
              <label>Genero:</label>
              <select
                name="gender"
                value={values.gender}
                onChange={handleChange}
              >
                <option value="hombre">Hombre</option>
                <option value="mujer">Mujer</option>
              </select>

              <label>Edad:</label>
              <input
                name="age"
                value={values.age}
                onChange={handleChange}
                type="number"
                placeholder="Edad del cliente"
                required
              />
              <span className="text-danger">{errors.age}</span>

              <label>Identificación:</label>
              <input
                name="identification"
                value={values.identification}
                onChange={handleChange}
                type="text"
                placeholder="Identificación del cliente"
              />
              <span className="text-danger">{errors.identification}</span>

              <label>Teléfono:</label>
              <input
                name="phone"
                value={values.phone}
                onChange={handleChange}
                type="text"
                placeholder="Teléfono del cliente"
              />
              <span className="text-danger">{errors.phone}</span>

              <label>Dirección:</label>
              <input
                name="address"
                value={values.address}
                onChange={handleChange}
                type="text"
                placeholder="Dirección del cliente"
              />
              <span className="text-danger">{errors.address}</span>

              <label>ID Cliente:</label>
              <input
                name="clientId"
                value={values.clientId}
                onChange={handleChange}
                type="text"
                placeholder="ID del cliente"
              />
              <span className="text-danger">{errors.clientId}</span>

              <label>Contraseña:</label>
              <input
                name="password"
                value={values.password}
                onChange={handleChange}
                type="password"
                placeholder="Contraseña del cliente"
              />
              <span className="text-danger">{errors.password}</span>

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

export default Users;
