export interface Property {
  id: string;
  title: string;
  description?: string;
  city: string;
  propertyType: string;
  price: number;
  squareMeters: number;
  auction?: Auction;
  imageUrl?: string;
  address?: string;
  bedrooms?: number;
  bathrooms?: number;
}

export interface Auction {
  id: string;
  propertyId: string;
  status: 'Active' | 'Scheduled' | 'Ended';
  startDate: string;
  endDate: string;
  startingPrice: number;
  currentPrice?: number;
  highestBidder?: string;
}

export interface PropertyListResponse {
  properties: Property[];
  total: number;
  page: number;
  pageSize: number;
}
