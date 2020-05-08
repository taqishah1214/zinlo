import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'formater'
})
export class Formater implements PipeTransform
{
    transform(value: any, args: any[]): any {
        if (!value) { return 0; }
        return new Intl.NumberFormat('en-US').format(value);
      }
}