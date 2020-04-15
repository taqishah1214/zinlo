export class PaymentDetails{
    cardNumber:string
    CVVCode:string
    expiryDate:string
    email:string
    commitment:string
    constructor()
    {
        this.cardNumber = '';
        this.CVVCode = "";
        this.expiryDate="";
        this.email="";
        this.commitment="";
    }
}