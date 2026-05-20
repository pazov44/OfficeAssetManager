# ==========================================
# STAGE 1: Build the React Application
# ==========================================
FROM node:20-alpine AS build
WORKDIR /app

# Copy package configurations first to leverage Docker layer caching
COPY package*.json ./

# Install all dependencies exactly as specified in your package-lock.json
RUN npm ci

# 🚨 CRITICAL: Copy the .env file explicitly so Vite can read your variables
COPY .env ./

# Copy the rest of your frontend source files
COPY . .

# Compile the React application into flat HTML/CSS/JS (outputs to /app/dist)
# Vite will actively look for the .env file during this execution line!
RUN npm run build

# ==========================================
# STAGE 2: Secure Production Runtime
# ==========================================
FROM nginx:stable-alpine AS runtime

# 🚨 FIXED: Copy your custom nginx routing configuration into Nginx's default config slot
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy the compiled flat assets from Stage 1 into Nginx's public directory
COPY --from=build /app/dist /usr/share/nginx/html

# Expose port 80 inside the container (Nginx default)
EXPOSE 80

# Start Nginx in the foreground so the container stays alive
CMD ["nginx", "-g", "daemon off;"]