import { defineConfig } from 'vite';
import { resolve } from 'path';
import fs from 'fs';

const replaceImportMap = (html) => {
  return html.replace(/<script[^<>]+importmap[^>]+>[^<>]+<\/script>\n*/i, '');
};

const transformHtml = () => {
  return {
    name: 'html-transform',
    transformIndexHtml(html) {
      return replaceImportMap(html);
    },
  };
};

const getImports = () => {
  const file = resolve(process.cwd(), 'index.html');
  const html = fs.readFileSync(file, 'utf-8');
  const importmapContent = html.match(/<script[^<>]+importmap[^>]+>([^<>]+)<\/script>\n*/)[1];
  const importmap = JSON.parse(importmapContent.replaceAll('./', '/'));
  console.log('importmap:');
  console.log(importmap);
  return importmap;
};

const alias = {};
Object.assign(alias, getImports().imports);

export default defineConfig({
  base: '/web-template-vanilla/',
  build: {
    target: 'esnext',
    module: 'esm',
  },
  resolve: {
    alias,
  },
  plugins: [transformHtml()],
});
