export interface Pagination {
  currentPage: number;
  itemPerPage: number;
  totalItems: number;
  totalPages: number;
}

export class PaginatedResult<T> {
  // @ts-ignore
  result: T;
  // @ts-ignore
  pagination: Pagination;
}
