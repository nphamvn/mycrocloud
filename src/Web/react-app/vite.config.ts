import { defineConfig, loadEnv } from "vite";
import react from "@vitejs/plugin-react";

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd());
  console.log(mode);
  console.log(env);
  return {
    plugins: [react()],
    server: {
      proxy: {
        "/api": {
          target: env.VITE_BASE_API_URL,
          changeOrigin: true,
        },
      },
    },
  };
});
