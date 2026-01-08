export type ToastType = 'success' | 'warning' | 'info';

export interface Toast {
  id: number;
  message: string;
  type: ToastType;
}
