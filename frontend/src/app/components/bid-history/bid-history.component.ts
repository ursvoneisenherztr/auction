import { Component, Input, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BidService } from '../../services/bid.service';
import { Bid } from '../../models/bid.model';
import { Auction } from '../../models/property.model';

@Component({
  selector: 'app-bid-history',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './bid-history.component.html',
  styleUrl: './bid-history.component.css'
})
export class BidHistoryComponent implements OnInit {
  @Input() auction: Auction | null = null;

  bids = signal<Bid[]>([]);
  loading = signal(false);
  error = signal('');
  currentPage = signal(1);
  readonly itemsPerPage = 20;

  constructor(private bidService: BidService) {}

  ngOnInit(): void {
    if (this.auction?.id) {
      this.loadBids();
    }
  }

  loadBids(): void {
    if (!this.auction?.id) return;

    this.loading.set(true);
    this.error.set('');

    this.bidService.getAuctionBids(this.auction.id).subscribe({
      next: (bids: Bid[]) => {
        const sortedBids = bids.sort((a, b) => {
          return new Date(b.bidTime).getTime() - new Date(a.bidTime).getTime();
        });
        this.bids.set(sortedBids);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading bids:', err);
        this.error.set('Failed to load bid history');
        this.loading.set(false);
      }
    });
  }

  getPaginatedBids(): Bid[] {
    const start = (this.currentPage() - 1) * this.itemsPerPage;
    const end = start + this.itemsPerPage;
    return this.bids().slice(start, end);
  }

  getTotalPages(): number {
    return Math.ceil(this.bids().length / this.itemsPerPage);
  }

  goToPage(page: number): void {
    const totalPages = this.getTotalPages();
    if (page >= 1 && page <= totalPages) {
      this.currentPage.set(page);
    }
  }

  nextPage(): void {
    if (this.currentPage() < this.getTotalPages()) {
      this.currentPage.update(p => p + 1);
    }
  }

  previousPage(): void {
    if (this.currentPage() > 1) {
      this.currentPage.update(p => p - 1);
    }
  }

  isHighestBid(bid: Bid): boolean {
    if (this.bids().length === 0) return false;
    return bid.bidId === this.bids()[0].bidId;
  }

  getRelativeTime(dateStr: string): string {
    const date = new Date(dateStr);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1) return 'just now';
    if (diffMins < 60) return `${diffMins} minute${diffMins > 1 ? 's' : ''} ago`;
    if (diffHours < 24) return `${diffHours} hour${diffHours > 1 ? 's' : ''} ago`;
    if (diffDays < 7) return `${diffDays} day${diffDays > 1 ? 's' : ''} ago`;

    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
