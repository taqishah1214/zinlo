import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'formater'
})
export class Formater implements PipeTransform
{
    transform(value: any, args: any[]): any {
        if (!value) { return ''; }
        return new Intl.NumberFormat('en-IN',{maximumSignificantDigits: 15 }).format(value);
      }
}