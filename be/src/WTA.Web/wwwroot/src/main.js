import html from './utils/index.js';
import { createApp } from 'vue';
import { createI18n } from 'vue-i18n';
import ElementPlus from 'element-plus';
import * as ElementPlusIconsVue from '@element-plus/icons-vue';
import store, { useAppStore } from './store/index.js';
import App from './app.js';

export default function (config) {
  const root = {
    components: { App, AppLayout: config.page.layout ?? config.layout, AppPage: config.page },
    template: html` <app>
      <app-layout>
        <app-page />
      </app-layout>
    </app>`,
  };
  // create app
  const app = createApp(root);
  // use store
  app.use(store);
  const appStore = useAppStore();
  appStore.settings.locale = config.locale.current;
  appStore.basePath = config.basePath;
  appStore.locale = config.locale;
  appStore.user = config.user;
  appStore.menus = config.menus;
  appStore.action = config.action;
  // use locale
  const localeConfig = {
    locale: appStore.locale.current,
    fallbackLocale: appStore.locale.default,
    allowComposition: true,
    messages: {
      [appStore.locale.current]: appStore.locale.resources,
    },
  };
  const i18n = createI18n(localeConfig);
  app.use(i18n);
  // use element plus
  app.use(ElementPlus);
  for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
    app.component(`Ep${key}`, component);
  }
  // mount
  app.mount(config.root);
}
