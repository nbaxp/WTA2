import html from '../utils/index.js';
import LayoutHeader from './layout-header.js';
import LayoutFooter from './layout-footer.js';
import { useAppStore } from '../store/index.js';

const template = html`<el-container class="h-100">
  <el-header class="el-header">
    <layout-header has-aside />
  </el-header>
  <el-container class="h-100" style="padding-top:60px;">
    <el-aside width="auto">
      <el-scrollbar>
        <el-menu :collapse="appStore.menuCollapse"> </el-menu>
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

  </el-container>
</el-container>`;

export default {
  components: { LayoutHeader, LayoutFooter },
  template,
  setup() {
    const appStore = useAppStore();
    return { appStore };
  },
};
