import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Bid, BidRequest, BidResponse } from '../models/bid.model';

@Injectable({
  providedIn: 'root'
})
export class BidService {
  private readonly apiUrl = 'http://localhost:7071/api/auctions';

  constructor(private http: HttpClient) {}

  placeBid(bidRequest: BidRequest): Observable<BidResponse> {
    return this.http.post<BidResponse>(`${this.apiUrl}/${bidRequest.auctionId}/bid`, bidRequest);
  }

  getAuctionBids(auctionId: string): Observable<Bid[]> {
    return this.http.get<Bid[]>(`${this.apiUrl}/${auctionId}/bids`);
  }

  getCurrentHighestBid(auctionId: string): Observable<Bid> {
    return this.http.get<Bid>(`${this.apiUrl}/${auctionId}/highest-bid`);
  }
}
