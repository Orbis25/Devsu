//validate with yup the schema GetUser
import * as yup from "yup";
 

export const validationSchema = yup.object().shape({
  name: yup.string().required("nombre requerido"),
  age: yup.number().positive("La edad debe ser un número positivo")
  .integer("La edad debe ser un número entero")
  .min(18, "La edad mínima es de 18 años")
  .required("Edad requerida"),
  identification: yup.string().required("identificacion requerida").matches(/^\d+$/, "Solo numeros"),
  phone: yup.string().required("telefono es requerido").matches(/^\d+$/, "Solo numeros"),
  clientId: yup.string().required("campo requerido"),
  password: yup.string().required("campo requerido").matches(/.{6,}/, "Campo Contraseña debe tener al menos 6 caracteres")
});
