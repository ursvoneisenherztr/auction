import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { PropertyService } from '../../services/property.service';
import { Property, Auction } from '../../models/property.model';
import { PlaceBidComponent } from '../place-bid/place-bid.component';
import { BidHistoryComponent } from '../bid-history/bid-history.component';

@Component({
  selector: 'app-property-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, PlaceBidComponent, BidHistoryComponent],
  templateUrl: './property-detail.component.html',
  styleUrl: './property-detail.component.css'
})
export class PropertyDetailComponent implements OnInit {
  property = signal<Property | null>(null);
  auction = signal<Auction | null>(null);
  loading = signal(false);
  error = signal('');

  constructor(
    private route: ActivatedRoute,
    private propertyService: PropertyService
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      const propertyId = params['id'];
      if (propertyId) {
        this.loadProperty(propertyId);
      }
    });
  }

  loadProperty(id: string): void {
    this.loading.set(true);
    this.error.set('');

    this.propertyService.getPropertyById(id).subscribe({
      next: (property: Property) => {
        this.property.set(property);
        
        if (property.auction?.id) {
          this.loadAuction(id);
        } else {
          this.loading.set(false);
        }
      },
      error: (err) => {
        console.error('Error loading property:', err);
        this.error.set('Failed to load property details. Please try again.');
        this.loading.set(false);
      }
    });
  }

  loadAuction(propertyId: string): void {
    this.propertyService.getAuctionByPropertyId(propertyId).subscribe({
      next: (auction: Auction) => {
        this.auction.set(auction);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading auction:', err);
        this.loading.set(false);
      }
    });
  }

  getAuctionStatusClass(status?: string): string {
    switch (status?.toLowerCase()) {
      case 'active':
        return 'status-active';
      case 'scheduled':
        return 'status-scheduled';
      case 'ended':
        return 'status-ended';
      default:
        return 'status-inactive';
    }
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
