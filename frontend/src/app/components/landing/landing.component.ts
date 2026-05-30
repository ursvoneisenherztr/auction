import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { PropertyService } from '../../services/property.service';
import { Property, PropertyListResponse } from '../../models/property.model';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.css']
})
export class LandingComponent implements OnInit {
  featuredProperties: Property[] = [];
  loading = false;
  stats = {
    totalProperties: 0,
    activeAuctions: 0,
    averagePrice: 0,
    successRate: 98
  };

  constructor(private propertyService: PropertyService) {}

  ngOnInit(): void {
    this.loadFeaturedProperties();
  }

  loadFeaturedProperties(): void {
    this.loading = true;
    this.propertyService.getProperties(1, 6).subscribe({
      next: (response: PropertyListResponse) => {
        this.featuredProperties = response.properties.slice(0, 6);
        this.stats.totalProperties = response.total;
        this.stats.activeAuctions = response.properties.filter(p => p.auction?.status === 'Active').length;
        this.stats.averagePrice = response.properties.length > 0 
          ? response.properties.reduce((sum, p) => sum + p.price, 0) / response.properties.length
          : 0;
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load properties:', err);
        this.loading = false;
      }
    });
  }

  formatPrice(price: number): string {
    return new Intl.NumberFormat('de-CH', {
      style: 'currency',
      currency: 'CHF',
      minimumFractionDigits: 0
    }).format(price);
  }

  getPropertyImage(propertyType: string): string {
    const images: { [key: string]: string } = {
      'house': '🏠',
      'apartment': '🏢',
      'land': '🌳',
      'commercial': '🏭',
      'other': '🏛️'
    };
    return images[propertyType?.toLowerCase()] || '🏛️';
  }
}
