FROM node:16
WORKDIR /app/quasar-project
COPY quasar-project/package*.json ./
RUN npm install -g @quasar/cli
RUN npm install
COPY quasar-project/. .
RUN npm run build
EXPOSE 9000
CMD ["quasar", "serve", "dist/spa", "--history", "--port", "9000"]