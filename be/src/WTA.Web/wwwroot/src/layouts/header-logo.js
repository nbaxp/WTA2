import html from '../utils/index.js';
import { useAppStore } from '../store/index.js';

export default {
  template: html`<a
    :href="appStore.basePath"
    class="d-flex justify-content-center align-items-center h-100"
    :class="{ aside: hasAside, collapse: appStore.menuCollapse }"
    style="min-width:44px;"
  >
    <el-space class="h-100">
      <img
        src="./assets/logo.svg"
        style="height:28px;"
      />
      <h1
        v-if="!appStore.menuCollapse"
        class="p-0 m-0 text-lg"
      >
        {{ $t('siteName') }}
      </h1>
    </el-space>
  </a>`,
  props: {
    hasAside: {
      type: Boolean,
      default: false,
    },
  },
  setup() {
    return {
      appStore: useAppStore(),
    };
  },
};
