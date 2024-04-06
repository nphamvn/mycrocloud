import { defineConfig, loadEnv } from "vite";
import react from "@vitejs/plugin-react";

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd());
  return {
    plugins: [react()],
    server: {
      proxy: {
        "/formapi": {
          target: env.VITE_BASE_FORM_API_URL,
          changeOrigin: true,
          rewrite: (path) => path.replace(/^\/formapi/, ""),
        },
      },
    },
    define: {
      __COMMIT_HASH__: JSON.stringify(process.env.COMMIT_HASH),
    },
  };
});
