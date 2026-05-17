export interface Product {
  id: string;
  name: string;
}

export interface DailyResult {
  date: string;
  product: Product;
}

export interface PeriodResult {
  from: string;
  to: string;
  product: Product;
}

export interface SizzlingHotResult {
  daily: DailyResult[],
  period: PeriodResult | null,
}