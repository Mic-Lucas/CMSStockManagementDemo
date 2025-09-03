import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';
import { StockService, StockItem } from '../../services/stock.service';

export interface StockFormData {
  stockItem?: StockItem;
}

@Component({
  selector: 'app-stock-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDialogModule,
    MatSnackBarModule,
    MatIconModule 
  ],
  templateUrl: './stock-form.html',
  styleUrls: ['./stock-form.scss']
})
export class StockForm implements OnInit {
  stockForm: FormGroup;
  imageFiles: File[] = [];
  imagePreviews: string[] = [];
  existingImages: { id: number, url: string }[] = []; 
  deletedImageIds: number[] = [];
  maxImages = 3;

  constructor(
    private fb: FormBuilder,
    private stockService: StockService,
    private snackBar: MatSnackBar,
    private dialogRef: MatDialogRef<StockForm>,
    @Inject(MAT_DIALOG_DATA) public data: StockFormData
  ) {
    this.stockForm = this.fb.group({
      make: ['', Validators.required],
      model: ['', Validators.required],
      modelYear: [null],
      kms: [null],
      colour: [''],
      vin: [''],
      retailPrice: [null],
      costPrice: [null],
      regNo: [''],
      accessories: this.fb.array([])
    });
  }

ngOnInit() {
  const stock = this.data?.stockItem;
  if (!stock) return;

  // Patch basic fields
  this.stockForm.patchValue({
    make: stock.make,
    model: stock.model,
    modelYear: stock.modelYear,
    kms: stock.kms,
    colour: stock.colour,
    vin: stock.vin,
    retailPrice: stock.retailPrice,
    costPrice: stock.costPrice,
    regNo: stock.regNo
  });

  // Load accessories from JSON (if any)
  if (stock.accessories && stock.accessories.length > 0) {
      stock.accessories.forEach((a: any) => this.addAccessory(a.name, a.description));
    }

  // Load existing images if the API provides URLs
 if (stock.images && stock.images.length > 0) {
  this.existingImages = [...stock.images]; // only existing images
  this.imagePreviews = stock.images.map((img: { url: string }) => img.url); // optional if you use previews
}
}

get accessories(): FormArray {
  return this.stockForm.get('accessories') as FormArray;
}

  addAccessory(name = '', description = '') {
    this.accessories.push(this.fb.group({ name, description }));
  }

  removeAccessory(index: number) {
    this.accessories.removeAt(index);
  }


onFilesSelected(event: Event) {
  const input = event.target as HTMLInputElement;
  if (!input.files) return;

  const files = Array.from(input.files);
  const totalImages = this.existingImages.length - this.deletedImageIds.length + this.imageFiles.length + files.length;

  if (totalImages > this.maxImages) {
    this.snackBar.open(`You can only upload up to ${this.maxImages} images`, '', { duration: 2000 });
    return;
  }

  files.forEach(file => {
    this.imageFiles.push(file);
    this.imagePreviews.push(URL.createObjectURL(file));
  });
}

removeImage(index: number, existing: boolean) {
  if (existing) {
    // Remove from existing images and track deletion
    const img = this.existingImages.splice(index, 1)[0];
    if (img.id) this.deletedImageIds.push(img.id);

    // Remove corresponding preview
    this.imagePreviews.splice(index, 1);
  } else {
    // Remove from new files
    this.imageFiles.splice(index, 1);

    // Remove corresponding preview (after existing images)
    this.imagePreviews.splice(this.existingImages.length + index, 1);
  }
}

  getFileUrl(file: File) {
    return URL.createObjectURL(file);
  }

  submit() {
    if (!this.stockForm.valid) return;

    const formData = new FormData();
    const value = this.stockForm.value;

    formData.append('Make', value.make);
    formData.append('Model', value.model);
    if (value.modelYear != null) formData.append('ModelYear', value.modelYear.toString());
    if (value.kms != null) formData.append('KMS', value.kms.toString());
    if (value.colour) formData.append('Colour', value.colour);
    if (value.vin) formData.append('VIN', value.vin);
    if (value.retailPrice != null) formData.append('RetailPrice', value.retailPrice.toString());
    if (value.costPrice != null) formData.append('CostPrice', value.costPrice.toString());
    if (value.regNo) formData.append('RegNo', value.regNo);

    // Accessories as JSON string
    if (value.accessories && value.accessories.length > 0) {
      formData.append('AccessoriesJson', JSON.stringify(value.accessories));
    }

    // Append up to 3 images
    this.imageFiles.slice(0, this.maxImages).forEach(file => formData.append('Images', file));

    if (this.deletedImageIds.length > 0) {
      formData.append('DeleteImageIdsCsv', this.deletedImageIds.join(','));
    }

    const request$ = this.data?.stockItem
      ? this.stockService.updateStock(this.data.stockItem.id, formData)
      : this.stockService.createStock(formData);

    request$.subscribe({
      next: () => {
        this.snackBar.open('Stock saved successfully', '', { duration: 1500 });
      // Clear new files and previews after successful save
      this.imageFiles = [];
      this.imagePreviews = [];
      this.deletedImageIds = [];
        this.dialogRef.close('saved');
      },
      error: (err) => {
        console.error(err);
        this.snackBar.open(err.error?.message || 'Failed to save stock', '', { duration: 2000 });
      }
    });
  }

  cancel() {
    this.dialogRef.close();
  }
}
