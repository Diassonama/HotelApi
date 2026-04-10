import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RackComponent } from './rack.component';
import { RackSignalRService } from './rack-signalr.service';

@NgModule({
  declarations: [
    RackComponent
  ],
  imports: [
    CommonModule
  ],
  providers: [
    RackSignalRService
  ],
  exports: [
    RackComponent
  ]
})
export class RackModule { }

// Para usar em standalone components (Angular 14+)
export { RackComponent, RackSignalRService };
