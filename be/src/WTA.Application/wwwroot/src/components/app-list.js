import html from '../utils/index.js';
import { reactive, onMounted, ref } from 'vue';

const template = html`
  <el-card>
    <el-row><el-col>查询表单</el-col> </el-row>
    <el-row><el-col>操作按钮</el-col> </el-row>
    <el-row>
      <el-col>
        <el-scrollbar>
          <el-table
            :ref="tableRef"
            row-key="id"
            border
            highlight-current-row
            table-layout="auto"
            :data="model.model.items"
            :default-sort="sortModel"
            :lazy="true"
            :load="lazyLoad"
            @sort-change="sortChange"
            @selection-change="onSelectionChange"
          >
            <el-table-column
              type="selection"
              align="center"
              fixed="left"
            />
            <el-table-column
              v-if="!model.disableRowIndex"
              :label="$t('rowIndex')"
              type="index"
              align="center"
              fixed="left"
            >
              <template #default="scope">
                {{ (pageModel.pageIndex - 1) * pageModel.pageSize + scope.$index + 1 }}
              </template>
            </el-table-column>
            <template
              v-for="(item, key) in model.schema.properties.items.items.properties"
              :key="key"
            >
              <template v-if="item.template !== 'hiddenInput'">
                <el-table-column
                  :key="key"
                  :prop="key"
                  :label="item.title ?? key"
                  :sortable="item.sortable ? (model.disablePagination ? true : 'custom') : null"
                >
                  <template #default="scope">
                    <app-form-input
                      :prop="key"
                      :model="scope.row"
                      :schema="item"
                      :disabled="true"
                    />
                    {{scope.row[key]}}
                  </template>
                </el-table-column>
              </template>
            </template>
            <slot name="operations">
              <el-table-column
                fixed="right"
                :label="$t('operations')"
              >
                <template #default="{ row }"> </template>
              </el-table-column>
            </slot>
          </el-table>
        </el-scrollbar>
      </el-col>
    </el-row>
    <el-row class="mt-4">
      <el-col>
        <el-pagination
          v-if="!model.disablePagination"
          v-model:currentPage="pageModel.pageIndex"
          v-model:page-size="pageModel.pageSize"
          class="justify-content-sm-end"
          :background="true"
          layout="total, sizes, prev, pager, next, jumper"
          :total="pageModel.total"
          @size-change="onPageSizeChange"
          @current-change="onPageIndexChange"
        />
      </el-col>
    </el-row>
  </el-card>
`;

export default {
  props: {
    modelValue: {
      type: Object,
      default: null,
    },
  },
  template,
  setup(props) {
    const model = reactive(props.modelValue);
    const pageModel = reactive({
      pageIndex: 1,
      pageSize: 10,
      total: 0,
    });
    return {
      model,
      pageModel,
    };
  },
};
