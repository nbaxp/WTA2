import html from '../utils/index.js';
import LayoutHeader from './layout-header.js';
import LayoutFooter from './layout-footer.js';

export default {
  components: { LayoutHeader, LayoutFooter },
  template: html`<el-container class="default-layout h-100">
    <el-scrollbar>
      <el-header class="el-header">
        <div class="block-container">
          <layout-header />
        </div>
      </el-header>
      <el-main class="el-main w-full">
        <div class="block-container">
          <slot />
        </div>
      </el-main>
      <el-footer>
        <div class="h-100">
          <layout-footer />
        </div>
      </el-footer>
      <el-backtop target=".default-layout > .el-scrollbar > .el-scrollbar__wrap" />
    </el-scrollbar>
  </el-container>`,
};
