import html from './utils/index.js';
import { useAppStore } from './store/index.js';
import en from '../libs/element-plus/locale/en.min.mjs';
import zh from '../libs/element-plus/locale/zh-cn.min.mjs';
import { watch, watchEffect } from 'vue';
import { useMediaQuery } from '@vueuse/core';

export default {
  template: html`<el-config-provider
    :size="appStore.settings.size"
    :button="{ autoInsertSpace: true }"
    :locale="currentLocale"
  >
    <slot />
  </el-config-provider>`,
  setup() {
    const locales = { en, zh };
    const appStore = useAppStore();
    const currentLocale = locales[appStore.settings.locale];

    const saveSettings = () => {
      localStorage.setItem('settings', JSON.stringify(appStore.settings));
    };

    watchEffect(() => {
      const darkClass = 'dark';
      const toDark = () => document.documentElement.classList.add(darkClass);
      const toLight = () => document.documentElement.classList.remove(darkClass);
      const isDarkNow = useMediaQuery('(prefers-color-scheme: dark)');

      if (appStore.settings.mode === 'auto') {
        isDarkNow.value ? toDark() : toLight();
      } else if (appStore.settings.mode === 'dark') {
        toDark();
      } else if (appStore.settings.mode === 'light') {
        toLight();
      }
      saveSettings();
    });

    watchEffect(() => {
      document.documentElement.style.setProperty('--el-color-primary', appStore.settings.themeColor);
      saveSettings();
    });

    watch(
      () => appStore.settings.locale,
      (newValue, oldValue) => {
        if (newValue !== oldValue) {
          saveSettings();
          var url = `${appStore.basePath}localization/set-language?culture=${newValue}`;
          fetch(url, { method: 'post' }).then(() => {
            window.location.reload();
          });
        }
      },
    );

    return {
      currentLocale,
      appStore,
    };
  },
};
