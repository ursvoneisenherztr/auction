import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { PropertyService } from '../../services/property.service';
import { Property, PropertyListResponse, Auction } from '../../models/property.model';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {
  // Navigation
  tabs = ['overview', 'properties', 'auctions', 'users', 'analytics'] as const;
  activeTab: typeof this.tabs[number] = 'overview';

  // Data
  properties: Property[] = [];
  auctions: Auction[] = [];
  loading = false;
  error: string | null = null;

  // Pagination
  currentPage = 1;
  pageSize = 10;
  totalItems = 0;

  // Stats
  stats = {
    totalProperties: 0,
    activeAuctions: 0,
    totalUsers: 3,
    totalBids: 0,
    averagePropertyPrice: 0,
    totalRevenue: 0
  };

  // Search & Filter
  searchQuery = '';
  filterStatus = '';
  editingProperty: Property | null = null;
  showPropertyForm = false;

  newProperty: any = {
    title: '',
    description: '',
    city: '',
    propertyType: '',
    price: 0,
    squareMeters: 0,
    bedrooms: 0,
    bathrooms: 0
  };

  constructor(private propertyService: PropertyService) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading = true;
    this.propertyService.getProperties(this.currentPage, this.pageSize).subscribe({
      next: (response: PropertyListResponse) => {
        this.properties = response.properties;
        this.totalItems = response.total;
        this.calculateStats();
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load data';
        this.loading = false;
      }
    });
  }

  calculateStats(): void {
    this.stats.totalProperties = this.properties.length;
    this.stats.activeAuctions = this.properties.filter(p => p.auction?.status === 'Active').length;
    this.stats.averagePropertyPrice = this.properties.length > 0 
      ? this.properties.reduce((sum, p) => sum + p.price, 0) / this.properties.length 
      : 0;
  }

  formatPrice(price: number): string {
    return new Intl.NumberFormat('de-CH', {
      style: 'currency',
      currency: 'CHF',
      minimumFractionDigits: 0
    }).format(price);
  }

  // Property Management
  openPropertyForm(): void {
    this.editingProperty = null;
    this.newProperty = {
      title: '',
      description: '',
      city: '',
      propertyType: '',
      price: 0,
      squareMeters: 0,
      bedrooms: 0,
      bathrooms: 0
    };
    this.showPropertyForm = true;
  }

  editProperty(property: Property): void {
    this.editingProperty = property;
    this.newProperty = { ...property };
    this.showPropertyForm = true;
  }

  saveProperty(): void {
    if (!this.newProperty.title || !this.newProperty.city) {
      alert('Please fill in all required fields');
      return;
    }
    // In a real app, this would call the API
    alert(this.editingProperty ? 'Property updated' : 'Property created');
    this.showPropertyForm = false;
    // Reload data
    this.loadData();
  }

  deleteProperty(propertyId: string): void {
    if (confirm('Are you sure you want to delete this property?')) {
      // In a real app, this would call the API
      alert('Property deleted');
      this.loadData();
    }
  }

  closePropertyForm(): void {
    this.showPropertyForm = false;
    this.editingProperty = null;
  }

  // Auction Management
  activateAuction(propertyId: string): void {
    alert(`Auction activated for property ${propertyId}`);
    this.loadData();
  }

  endAuction(propertyId: string): void {
    alert(`Auction ended for property ${propertyId}`);
    this.loadData();
  }

  // Filtering
  getFilteredProperties(): Property[] {
    return this.properties.filter(p => {
      const searchMatch = !this.searchQuery || 
        p.title.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        p.city.toLowerCase().includes(this.searchQuery.toLowerCase());
      
      const statusMatch = !this.filterStatus || p.auction?.status === this.filterStatus;
      
      return searchMatch && statusMatch;
    });
  }

  // Pagination
  nextPage(): void {
    this.currentPage++;
    this.loadData();
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadData();
    }
  }

  getTotalPages(): number {
    return Math.ceil(this.totalItems / this.pageSize);
  }
}
