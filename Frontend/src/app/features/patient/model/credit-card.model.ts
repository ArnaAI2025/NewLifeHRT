export interface CreditCardDto {
  id?: string;
  last4: string;
  cardType: number;
  month: number;    
  year: string;    
  maskedNumber: string;
  cardNumber : string;
}

