import html from './utils/index.js';
import { createApp } from 'vue';
import { createI18n } from 'vue-i18n';
import ElementPlus from 'element-plus';
import * as ElementPlusIconsVue from '@element-plus/icons-vue';
import store, { useAppStore } from './store/index.js';
import { globalConfig } from './request/index.js';
import App from './app.js';

export default function (config) {
  globalConfig.baseURL = config.basePath;
  const root = {
    components: { App, AppLayout: config.page.layout ?? config.layout, AppPage: config.page },
    template: html` <app>
      <app-layout>
        <app-page />
      </app-layout>
    </app>`,
  };
  //dayjs
  dayjs.locale(window.dayjs_locale_zh_cn);
  //signalr
  const connection = new signalR.HubConnectionBuilder().withUrl(`${globalConfig.baseURL}hub`).build();
  const connect = () => {
    if (connection.state === signalR.HubConnectionState.Disconnected) {
      connection
        .start()
        .then(function () {
          console.log('signalr connected');
        })
        .catch(function (error) {
          console.error(error);
          setTimeout(connect, 5000);
        });
    }
  };
  connection.onclose(function () {
    connect();
  });
  connection.on('Connected', function (id) {
    window.connectionId = id;
    //PubSub.publish('Connected');
  });
  connection.on('ServerToClient', function (method, message, to, from) {
    PubSub.publish(method, { message: message, to: to, from: from });
  });
  if (config.user.isAuthenticated) {
    connect();
  }
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
