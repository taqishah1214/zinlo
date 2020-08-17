FROM node:13-alpine as Builder

ARG APP_API_URL
ENV APP_API_URL $APP_API_URL

WORKDIR /app/

COPY package*.json ./

RUN yarn install

COPY . .

RUN yarn run publish

######################################################################
FROM nginx:alpine

COPY --from=Builder /app/dist /usr/share/nginx/html/
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

