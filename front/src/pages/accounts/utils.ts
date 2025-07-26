//validate with yup the schema GetUser
import * as yup from "yup";

export const validationSchema = yup.object().shape({
  accountNumber: yup.string().required("campo requerido")
  .matches(/^\d+$/, "Solo numeros"),
  initialBalance: yup
    .number()
    .required("campo requerido")
    .min(1, "El balance inicial debe ser mayor a 0"),
});
