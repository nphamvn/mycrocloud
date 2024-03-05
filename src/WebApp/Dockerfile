FROM node:21-alpine3.18 as build
WORKDIR /app
COPY package.json ./
RUN npm install
COPY . .
ARG COMMIT_HASH
ENV COMMIT_HASH=$COMMIT_HASH
RUN npm run build

FROM nginx:1.25-alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]