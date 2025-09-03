import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface StockItemListDto {
  id: number;
  make: string;
  model: string;
  modelYear?: number;
  retailPrice?: number;
  primaryImageUrl?: string;
}

export interface StockPagedResponse {
  total: number;
  page: number;
  pageSize: number;
  data: StockItemListDto[];
}

export interface StockImage {
  id: number;
  url: string;
}

export interface StockItem {
    id: number;
    make: string;
    model: string;
    modelYear?: number;
    retailPrice?: number;
    primaryImageUrl?: string;
    kms? : number;
    colour? : number;
    vin? : number;
    costPrice? : number;
    regNo : string;
    accessoriesJson?: string;      // for API
    accessories?: { name: string; description: string }[]; // for UI
    images?: StockImage[];
  }

@Injectable({
  providedIn: 'root'
})
export class StockService {
  private baseUrl = 'http://localhost:5000/api/stock';

  constructor(private http: HttpClient) {}

  /** Get paged stock list with search/sort */
  getPaged(
    page = 1,
    pageSize = 10,
    sortBy = 'DTUpdated',
    sortDir: 'asc' | 'desc' = 'desc',
    search: string | null = null
  ): Observable<StockPagedResponse> {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize)
      .set('sortBy', sortBy)
      .set('sortDir', sortDir);

    if (search) params = params.set('search', search);

    return this.http.get<StockPagedResponse>(this.baseUrl, { params });
  }

  /** Get single stock item by ID */
  getById(id: number): Observable<StockItemListDto> {
    return this.http.get<StockItemListDto>(`${this.baseUrl}/${id}`);
  }

   // Create new stock item
   createStock(formData: FormData): Observable<any> {
    return this.http.post(`${this.baseUrl}`, formData);
  }

  // Update existing stock item
  updateStock(id: number, formData: FormData): Observable<any> {
    return this.http.put(`${this.baseUrl}/${id}`, formData);
  }

  // Delete stock item
  deleteStock(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }
}
