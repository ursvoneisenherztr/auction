import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { PropertyService } from '../../services/property.service';
import { Property } from '../../models/property.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  properties: Property[] = [];
  filteredProperties: Property[] = [];
  loading = false;
  error: string | null = null;

  // Filters
  selectedCity = '';
  selectedType = '';
  searchQuery = '';
  priceRange = 1000000;

  // Stats
  totalProperties = 0;
  activeAuctions = 0;
  cities: string[] = [];
  propertyTypes: string[] = [];

  constructor(private propertyService: PropertyService) {}

  ngOnInit(): void {
    this.loadProperties();
  }

  loadProperties(): void {
    this.loading = true;
    this.propertyService.getProperties(1, 100).subscribe({
      next: (response) => {
        this.properties = response.properties;
        this.filteredProperties = response.properties;
        this.totalProperties = response.total;
        this.activeAuctions = response.properties.filter(p => p.auction?.status === 'Active').length;
        this.extractFilterOptions();
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load properties';
        this.loading = false;
      }
    });
  }

  extractFilterOptions(): void {
    this.cities = [...new Set(this.properties.map(p => p.city))].sort();
    this.propertyTypes = [...new Set(this.properties.map(p => p.propertyType))].sort();
  }

  applyFilters(): void {
    this.filteredProperties = this.properties.filter((p: Property) => {
      const cityMatch = !this.selectedCity || p.city === this.selectedCity;
      const typeMatch = !this.selectedType || p.propertyType === this.selectedType;
      const priceMatch = p.price <= this.priceRange;
      const searchMatch = !this.searchQuery || 
        p.title.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        p.city.toLowerCase().includes(this.searchQuery.toLowerCase());
      
      return cityMatch && typeMatch && priceMatch && searchMatch;
    });
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  onSearch(query: string): void {
    this.searchQuery = query;
    this.applyFilters();
  }

  formatPrice(price: number): string {
    return new Intl.NumberFormat('de-CH', {
      style: 'currency',
      currency: 'CHF',
      minimumFractionDigits: 0
    }).format(price);
  }

  getAuctionStatusClass(status: string | undefined): string {
    switch (status) {
      case 'Active':
        return 'status-active';
      case 'Scheduled':
        return 'status-scheduled';
      case 'Ended':
        return 'status-ended';
      default:
        return 'status-unknown';
    }
  }

  getFeaturedProperties(): Property[] {
    return this.filteredProperties.slice(0, 6);
  }
}
