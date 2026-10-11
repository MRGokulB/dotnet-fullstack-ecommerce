export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        background: '#07090E',
        surface: '#0E131F',
        border: '#1F2937',
        accent: {
          500: '#6366F1',
          600: '#4F46E5',
          700: '#4338CA'
        }
      }
    },
  },
  plugins: [],
}
