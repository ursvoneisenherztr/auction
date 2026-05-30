export interface Bid {
  bidId: string;
  auctionId: string;
  bidderId: string;
  amount: number;
  bidTime: string;
}

export interface BidRequest {
  auctionId: string;
  bidderId: string;
  amount: number;
}

export interface BidResponse {
  success: boolean;
  message: string;
  bid?: Bid;
  error?: string;
}
