import { createI18n } from 'vue-i18n';

const config = {
  locale: appStore.locale,
  fallbackLocale: 'zh',
  allowComposition: true,
  messages: {
    en,
    zh,
  },
};

const i18n = createI18n(config);

export default i18n;
