import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Property, PropertyListResponse, Auction } from '../models/property.model';

@Injectable({
  providedIn: 'root'
})
export class PropertyService {
  private readonly apiUrl = 'http://localhost:7071/api/properties';

  constructor(private http: HttpClient) {}

  getProperties(page: number = 1, pageSize: number = 10, filters?: { city?: string; propertyType?: string }): Observable<PropertyListResponse> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (filters?.city) {
      params = params.set('city', filters.city);
    }
    if (filters?.propertyType) {
      params = params.set('propertyType', filters.propertyType);
    }

    return this.http.get<PropertyListResponse>(this.apiUrl, { params });
  }

  getPropertyById(id: string): Observable<Property> {
    return this.http.get<Property>(`${this.apiUrl}/${id}`);
  }

  getAuctionByPropertyId(propertyId: string): Observable<Auction> {
    return this.http.get<Auction>(`${this.apiUrl}/${propertyId}/auction`);
  }
}
