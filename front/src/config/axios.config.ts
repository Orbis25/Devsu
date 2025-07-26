import { toast } from "react-toastify";
import {
  type IProblemDetail,
  isBadResponse,
} from "../shared/interfaces/base-http-response.interface";
import axios from "axios";

//axios config
const axiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
});

axiosInstance.interceptors.response.use(
  (response) => response,
  async (error) => {
    // Handle error response
    if (error.response && error.response.status === 400) {
      const badResponse = error.response.data;
      console.log(badResponse);
      const isBad = isBadResponse(badResponse);

      if (isBad) {
        toast(badResponse.message, { type: "error" });
      } else {

        const _bad = badResponse as IProblemDetail;
        const keys = Object.keys(_bad.errors);
        const messages = keys
          .map((key) => `${key}: ${_bad.errors[key].join(", ")}`)
          .join("\n");

        toast(messages, { type: "error" });
      }
    }

    if(error.response && error.response.status === 404){
              const {message} = error.response.data;
        toast(message, { type: "warning" });

    }
    return Promise.reject(new Error(error));
  }
);

export default axiosInstance;
