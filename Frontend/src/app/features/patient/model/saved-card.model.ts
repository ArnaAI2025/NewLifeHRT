export interface SavedCard {
  id: string | number;
  last4: string;
  expiryMonth: number;
  expiryYear: string;
  cardType: number;
  cardToken: string;
  creditCardNumber?: string;
  cardNumber?: string;
  maskedNumber: string;
}
