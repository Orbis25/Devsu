import { IPaginationResult } from "@/shared/interfaces/pagination.interface";
import React from "react";

export interface Column<T> {
  key: keyof T | "actions";
  label: string;
  render?: (value: unknown, row: T) => React.ReactNode;
}

interface GenericTableProps<T> {
  columns: Column<T>[];
  data: IPaginationResult<T>;
  currentPage: number;
  hideActions?: boolean;
  onRowClick?: (row: T) => void;
  onChangePage?: (page: number) => void;
}

export default function GenericTable<T>({
  columns,
  data,
  currentPage,
  onRowClick,
  onChangePage,
  hideActions = false,
}: GenericTableProps<T>) {

  const { results, total, pageTotal, qyt } = data;


  const handleChangePage = (page: number) => {
     if (onChangePage) {
      onChangePage(page);
    }
  }

  const getColumns = () => {
    if( hideActions) {
      return columns.filter(col => col.key !== "actions");
    }
    return columns;
  }


  return (
    <>
      <table className="table">
        <thead>
          <tr>
            {getColumns().map((col) => (
              <th key={String(col.label)}>{col.label}</th>
            ))}
          </tr>
        </thead>
        <tbody>
          {results.map((row, i) => (
            <tr
              key={i}
              onClick={() => onRowClick?.(row)}
              className={`table-row ${onRowClick ? "clickable" : ""}`}
            >
              {getColumns().map((col) => (
                <td key={String(col.label)}>
                  {col.render
                    ? col.render(
                        col.key === "actions" ? null : row[col.key as keyof T],
                        row
                      )
                    : col.key === "actions"
                    ? null
                    : String(row[col.key as keyof T])}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
      <span>
        Mostrando {total} resultados, {qyt} por página
      </span>{" "}
      <br />
      <div className="pagination">
        {Array.from({ length: pageTotal }, (_, i) => (
          <button
            key={i}
            onClick={() => handleChangePage(i + 1)}
            className={`pagination-button ${
              currentPage === i + 1 ? "active" : ""
            }`}
          >
            {i + 1}
          </button>
        ))}
      </div>
    </>
  );
}
