import html from './utils/index.js';
import { useAppStore } from './store/index.js';
import en from '../libs/element-plus/locale/en.min.mjs';
import zh from '../libs/element-plus/locale/zh-cn.min.mjs';

export default {
  template: html`<el-config-provider
    :size="appStore.size"
    :button="{ autoInsertSpace: true }"
    :locale="currentLocale"
  >
    <slot />
  </el-config-provider>`,
  setup() {
    const locales = { en, zh };
    const appStore = useAppStore();
    const currentLocale = locales[appStore.locale.current];
    return {
      currentLocale,
      appStore,
    };
  },
};
