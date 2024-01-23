/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ['./**/*.{razor,html}'],
  theme: {
    extend: {
      fontFamily: {
        'body': ["Comfortaa", "sans-serif"],
      },
    },
  },
  plugins: [],
}

// npx tailwindcss -i ./Styles/app.css -o ./wwwroot/css/tailwind.css --watch