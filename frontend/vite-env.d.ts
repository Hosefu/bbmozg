/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_URL: string;
  // добавьте здесь другие переменные окружения
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}