import html from '../utils/index.js';
import LayoutHeader from './layout-header.js';
import LayoutFooter from './layout-footer.js';
import MenuItem from './menu-item.js';
import { useAppStore } from '../store/index.js';
import Enumerable from '../../libs/linq/linq.min.js';
import { computed } from 'vue';

const template = html`<el-container class="h-100">
  <el-header class="el-header">
    <layout-header has-aside />
  </el-header>
  <el-container
    class="h-100"
    style="padding-top:60px;"
  >
    <el-aside width="auto">
      <el-scrollbar>
        <el-menu
          :collapse-transition="false"
          :collapse="appStore.menuCollapse"
          :default-active="appStore.action"
        >
          <menu-item v-model="subMenus" />
        </el-menu>
      </el-scrollbar>
    </el-aside>
    <el-container class="main">
      <el-scrollbar>
        <el-main class="el-main w-100">
          <slot />
        </el-main>
        <el-footer>
          <layout-footer />
        </el-footer>
        <el-backtop target=".main > .el-scrollbar > .el-scrollbar__wrap" />
      </el-scrollbar>
    </el-container>
  </el-container>
</el-container>`;

export default {
  components: { LayoutHeader, LayoutFooter, MenuItem },
  template,
  setup() {
    const appStore = useAppStore();
    const subMenus = computed(() => {
      if (appStore.useTopMenus) {
        return Enumerable.from(appStore.menus).firstOrDefault((o) => appStore.action.indexOf(o.url) === 0).children;
      } else {
        return appStore.menus;
      }
    });
    return { appStore, subMenus };
  },
};
