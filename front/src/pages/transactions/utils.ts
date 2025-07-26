//validate with yup the schema GetUser
import * as yup from "yup";
 

export const validationSchema = yup.object().shape({
  type: yup.string().required("campo requerido"),
  accountId: yup.string().required("campo requerido"),

  amount: yup
    .number()
    .positive("Debe ser un número positivo")
    .min(1, "El monto debe ser mayor a 1")
    .required("campo requerido"),
});
