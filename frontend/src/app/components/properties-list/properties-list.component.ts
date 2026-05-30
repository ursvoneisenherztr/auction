import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { PropertyService } from '../../services/property.service';
import { Property, PropertyListResponse } from '../../models/property.model';

@Component({
  selector: 'app-properties-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './properties-list.component.html',
  styleUrl: './properties-list.component.css'
})
export class PropertiesListComponent implements OnInit {
  properties = signal<Property[]>([]);
  loading = signal(false);
  error = signal('');
  
  currentPage = signal(1);
  pageSize = 10;
  totalProperties = signal(0);
  
  searchCity = signal('');
  searchPropertyType = signal('');
  availablePropertyTypes = ['Apartment', 'House', 'Commercial', 'Land'];
  
  // Expose Math for template use
  readonly Math = Math;

  constructor(private propertyService: PropertyService) {}

  ngOnInit(): void {
    this.loadProperties();
  }

  loadProperties(): void {
    this.loading.set(true);
    this.error.set('');
    
    const filters = {
      city: this.searchCity() || undefined,
      propertyType: this.searchPropertyType() || undefined
    };

    this.propertyService.getProperties(this.currentPage(), this.pageSize, filters)
      .subscribe({
        next: (response: PropertyListResponse) => {
          this.properties.set(response.properties);
          this.totalProperties.set(response.total);
          this.loading.set(false);
        },
        error: (err) => {
          console.error('Error loading properties:', err);
          this.error.set('Failed to load properties. Please try again.');
          this.loading.set(false);
        }
      });
  }

  onSearch(): void {
    this.currentPage.set(1);
    this.loadProperties();
  }

  onReset(): void {
    this.searchCity.set('');
    this.searchPropertyType.set('');
    this.currentPage.set(1);
    this.loadProperties();
  }

  goToPage(page: number): void {
    const totalPages = Math.ceil(this.totalProperties() / this.pageSize);
    if (page >= 1 && page <= totalPages) {
      this.currentPage.set(page);
      this.loadProperties();
    }
  }

  get totalPages(): number {
    return Math.ceil(this.totalProperties() / this.pageSize);
  }

  get paginationArray(): number[] {
    const pages: number[] = [];
    const totalPages = this.totalPages;
    const currentPage = this.currentPage();
    
    let start = Math.max(1, currentPage - 2);
    let end = Math.min(totalPages, start + 4);
    
    if (end - start < 4) {
      start = Math.max(1, end - 4);
    }
    
    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    return pages;
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

  getAuctionStatusText(status?: string): string {
    return status || 'No Auction';
  }
}
