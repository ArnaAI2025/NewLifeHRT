export interface HttpRequestOptions {
  attachToken?: boolean;
  headers?: Record<string, string>;
  timeout?: number;
  skipEncryption?: boolean;
  params?: Record<string, string | number | boolean | string[]>;
  responseType?: 'json' | 'text' | 'blob';
  observe?: 'body' | 'response' | 'events';
}



export const DefaultHttpRequestOptions: HttpRequestOptions = {
  attachToken: true,
  timeout: 10000,
  skipEncryption: false,
  headers: {},
  params: {},
  responseType: 'json',
  observe: 'body'
};
