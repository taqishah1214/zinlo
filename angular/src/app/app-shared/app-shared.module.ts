import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FirstLetterOfWords } from '@shared/customPipe/firstLetterOfWords';



@NgModule({
  declarations: [
    FirstLetterOfWords
  ],
  imports: [
    CommonModule
  ],
  exports : [
    FirstLetterOfWords
  ]
})
export class AppSharedModule { }
