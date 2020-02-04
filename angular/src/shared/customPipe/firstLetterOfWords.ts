import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'firstLetterOfWords'
})
export class FirstLetterOfWords implements PipeTransform
{
    transform(value: string, args: any[]): string {
        if (!value) { return ''; }
        return value.split(' ').map(function(item){return item[0]}).join('').toUpperCase();
      }
}