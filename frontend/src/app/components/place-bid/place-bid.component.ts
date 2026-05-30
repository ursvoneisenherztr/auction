import { Component, Input, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BidService } from '../../services/bid.service';
import { BidRequest, Bid } from '../../models/bid.model';
import { Auction } from '../../models/property.model';

@Component({
  selector: 'app-place-bid',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './place-bid.component.html',
  styleUrl: './place-bid.component.css'
})
export class PlaceBidComponent implements OnInit {
  @Input() auction: Auction | null = null;

  bidForm!: FormGroup;
  loading = signal(false);
  success = signal('');
  error = signal('');
  currentHighestBid = signal<Bid | null>(null);

  constructor(private fb: FormBuilder, private bidService: BidService) {
    this.bidForm = this.fb.group({
      bidderId: ['', [Validators.required]],
      amount: ['', [Validators.required, Validators.min(0.01)]]
    });
  }

  ngOnInit(): void {
    if (this.auction?.id) {
      this.loadHighestBid();
    }
  }

  loadHighestBid(): void {
    if (!this.auction?.id) return;

    this.bidService.getCurrentHighestBid(this.auction.id).subscribe({
      next: (bid: Bid) => {
        this.currentHighestBid.set(bid);
        this.validateBidAmount();
      },
      error: (err) => {
        console.log('No bids found yet or error loading highest bid');
        this.currentHighestBid.set(null);
      }
    });
  }

  validateBidAmount(): void {
    const amountControl = this.bidForm.get('amount');
    if (!amountControl) return;

    const minBidAmount = this.getMinimumBidAmount();
    amountControl.setValidators([
      Validators.required,
      Validators.min(minBidAmount)
    ]);
    amountControl.updateValueAndValidity();
  }

  getMinimumBidAmount(): number {
    if (this.currentHighestBid()) {
      return this.currentHighestBid()!.amount + 1;
    }
    return this.auction?.startingPrice || 1;
  }

  isAuctionActive(): boolean {
    return this.auction?.status === 'Active';
  }

  get amountError(): string {
    const control = this.bidForm.get('amount');
    if (!control || !control.touched) return '';

    if (control.hasError('required')) {
      return 'Bid amount is required';
    }
    if (control.hasError('min')) {
      return `Bid must be at least $${this.getMinimumBidAmount().toFixed(2)}`;
    }
    return '';
  }

  get bidderIdError(): string {
    const control = this.bidForm.get('bidderId');
    if (!control || !control.touched) return '';
    return control.hasError('required') ? 'Bidder ID/Name is required' : '';
  }

  onSubmit(): void {
    if (!this.bidForm.valid || !this.auction?.id || !this.isAuctionActive()) {
      return;
    }

    this.loading.set(true);
    this.error.set('');
    this.success.set('');

    const bidRequest: BidRequest = {
      auctionId: this.auction.id,
      bidderId: this.bidForm.get('bidderId')?.value,
      amount: parseFloat(this.bidForm.get('amount')?.value)
    };

    this.bidService.placeBid(bidRequest).subscribe({
      next: (response) => {
        this.loading.set(false);
        if (response.success) {
          this.success.set('Bid placed successfully!');
          this.bidForm.reset();
          setTimeout(() => {
            this.success.set('');
          }, 5000);
          this.loadHighestBid();
        } else {
          this.error.set(response.message || 'Failed to place bid');
        }
      },
      error: (err) => {
        this.loading.set(false);
        console.error('Error placing bid:', err);
        this.error.set(err.error?.message || 'Failed to place bid. Please try again.');
      }
    });
  }
}
