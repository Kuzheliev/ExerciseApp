import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { quoteDetail } from 'src/app/Model/QuoteDetail';
import { quoteResponse } from 'src/app/Model/QuoteResponse';
import { UnsureService } from 'src/app/service/unsure.service';

@Component({
  selector: 'unsure',
  templateUrl: './unsure.component.html',
  styleUrls: ['./unsure.component.scss'],
  standalone: false
})
export class UnsureComponent implements OnInit {

  constructor(private unsureService: UnsureService ) {
    this.touched = false;
   }

  touched: boolean;
  details: quoteDetail = new quoteDetail();
  Models: Array<string> = [];
  Quote: number= 0;
  QuoteReceived: boolean = false;
  ErrorMessage: string = '';
  ShowError: boolean = false;
  isLoading: boolean = false;
  calculatedAge: number = 0;

  ngOnInit(): void {
    this.unsureService.getDetails<quoteDetail>().subscribe(s=>{
      this.details.makes = s.makes;
      this.details.models = s.models;
      this.details.insuranceTypes = s.insuranceTypes;
      this.setModels();
    });
  }


  dateOfBirthCtrl= new FormControl('', Validators.required);
  makeCtrl = new FormControl('',Validators.required);
  modelCtrl = new FormControl('',Validators.required);
  insuranceTypeCtrl = new FormControl('',Validators.required);

  quoteForm = new FormGroup({
    dateOfBirth: this.dateOfBirthCtrl,
    make: this.makeCtrl,
    model: this.modelCtrl,
    insuranceType: this.insuranceTypeCtrl,
  });

  public changeMake(e: any) {
    this.quoteForm.controls["make"].setValue(e.target.value, {  onlySelf: true  })
    this.setModels();
  }

  public changeModel(e: any) {
    this.quoteForm.controls["model"].setValue(e.target.value, {  onlySelf: true  })
  }

  public changeInsuranceType(e: any) {
    this.quoteForm.controls["insuranceType"].setValue(e.target.value, {  onlySelf: true  })
  }

  /**
   * Calculate age from date of birth
   */
  private calculateAge(dateOfBirth: string): number {
    const birth = new Date(dateOfBirth);
    const today = new Date();

    let age = today.getFullYear() - birth.getFullYear();
    const monthDiff = today.getMonth() - birth.getMonth();

    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birth.getDate())) {
      age--;
    }

    return age;
  }

  /**
   * Handle date of birth change
   */
  public onDateOfBirthChange(): void {
    const dobValue = this.dateOfBirthCtrl.value;
    if (dobValue) {
      this.calculatedAge = this.calculateAge(dobValue);

      // Persist calculated age to model if needed
      console.log(`Age calculated: ${this.calculatedAge} years`);
    }
  }

  /**
   * Check if age is within eligibility range (17-80)
   */
  private isAgeInRange(age: number): boolean {
    return age >= 17 && age <= 80;
  }

  /**
   * Public method to check if calculated age is eligible (for template binding)
   */
  public isAgeEligible(): boolean {
    return this.calculatedAge >= 17 && this.calculatedAge <= 80;
  }

  /**
   * Validate age on client side before sending to backend
   */
  private validateAgeClientSide(): string | null {
    const dobValue = this.dateOfBirthCtrl.value;

    if (!dobValue) {
      return 'Date of birth is required.';
    }

    const age = this.calculateAge(dobValue);

    if (age < 17) {
      return 'You must be at least 17 years old to request a quote.';
    }

    if (age > 80) {
      return 'You must be 80 years old or younger to request a quote.';
    }

    return null;
  }

  setModels(){
    let currentMake = this.quoteForm.controls["make"].value;
    this.Models = [];
    let makes = this.details.models.filter(function(m){return m.make == currentMake});
    if (makes.length > 0)
      this.Models = makes[0].models;
  }

  /**
   * Get Quote from backend
   */
  GetQuote()
  {
    this.touched = true;

    // Clear previous errors
    this.ShowError = false;
    this.ErrorMessage = '';
    this.QuoteReceived = false;

    // Validate form fields
    if (this.quoteForm.invalid && this.touched)
    {
      return; // Form validation will be displayed
    }

    // Validate age on client side first
    const ageError = this.validateAgeClientSide();
    if (ageError) {
      this.ShowError = true;
      this.ErrorMessage = ageError;
      return;
    }

    // Show loading state
    this.isLoading = true;

    // Call backend
    this.unsureService.GetQuote<quoteResponse>(this.quoteForm.value).subscribe(
      s => {
        this.isLoading = false;

        if (s.quoteRequestValid)
        {
          // Quote is valid - show the quote
          this.QuoteReceived = true;
          this.Quote = s.quote;
          this.ShowError = false;
          this.ErrorMessage = '';
        }
        else
        {
          // Quote request failed - show error message
          this.QuoteReceived = false;
          this.Quote = 0;
          this.ShowError = true;
          this.ErrorMessage = s.errorMessage || 'Unable to process your quote request. Please try again.';
        }
        this.setModels();
      },
      error => {
        // Handle network or server errors
        this.isLoading = false;
        this.QuoteReceived = false;
        this.ShowError = true;
        this.ErrorMessage = 'An error occurred while retrieving your quote. Please try again.';
        console.error('Quote request error:', error);
      }
    );
  }

  /**
   * Dismiss age error message
   */
  public dismissAgeError(): void {
    this.ShowError = false;
    this.ErrorMessage = '';
  }

  /**
   * Reset form and prepare for new quote
   */
  public resetForm(): void {
    this.quoteForm.reset();
    this.touched = false;
    this.QuoteReceived = false;
    this.ShowError = false;
    this.ErrorMessage = '';
    this.calculatedAge = 0;
    this.isLoading = false;
  }
}
