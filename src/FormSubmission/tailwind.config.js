/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: "#0891b2",
        secondary: "#718096",
        tertiary: "#e2e8f0",
        quaternary: "#f7fafc",
        quinary: "#ffffff",
      },
    },
  },
  plugins: [],
}

