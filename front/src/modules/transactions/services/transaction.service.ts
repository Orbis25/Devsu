import { IPaginationResult } from "../../../shared/interfaces/pagination.interface";
import {
  BaseResponse,
  BaseService,
} from "../../../shared/services/base.service";
import { GetTransaction } from "../interfaces/get-transaction";

export class TransactionService extends BaseService {
  /**
   *
   */
  constructor() {
    super("transactions");
  }

  async search(search = { page: 1, qyt: 10,noPaginate: false, from: "", to: "", query: "" }) {
    const queryParams = new URLSearchParams();

    queryParams.append("noPaginate", search.noPaginate.toString());
    queryParams.append("page", search.page.toString());
    queryParams.append("qyt", search.qyt.toString());
    queryParams.append("query", search.query);
    if (search.from) {
      queryParams.append("from", search.from);
    }
    if (search.to) {
      queryParams.append("to", search.to);
    }

    const response = await this.axiosInstance.get<
      BaseResponse<IPaginationResult<GetTransaction>>
    >(`${this.baseEndpoint}/report`, {
      params: queryParams,
    });

    return response;
  }

  async exportFile(search = { page: 1, from: "", to: "", type: "pdf" }) {
    const queryParams = new URLSearchParams();

    queryParams.append("page", search.page.toString());
    queryParams.append("noPaginate", "true");
    if (search.from) {
      queryParams.append("from", search.from);
    }
    if (search.to) {
      queryParams.append("to", search.to);
    }

    const response = await this.axiosInstance.get<
      BaseResponse<string>
    >(`${this.baseEndpoint}/report/${search.type}`, {
      params: queryParams,
    });

    return response;
  }
}
