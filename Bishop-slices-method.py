from PreProcess import *
from AfterProcess import *

# 边坡尺寸参数设定
H = 5
alpha_slope = 45
# 土质参数
c_effective = 5 # 黏聚力
phi_effective = 30 # 内摩擦角
gamma = 20 # 重度
# 条分数目
N = 20
# 矩形区域单边布种数量
seed = 5
# 计算次数标记
calc_num = 1
#  delta_R:0-H m 
delta_R = 3

# 主计算程序 
# 初始化几何参数
c = coordinate(H,alpha_slope,c_effective,phi_effective,N)
center_list = circle_center(H,c['A_x'],c['A_y'],seed)
Fs_min = 999
Fs_min_list = []
# 二层循环center,delta_R
for center in center_list:
	print(center)
	Fs_min_each = 999 
	R_0 = circle_radius(center,c['B_x'],c['B_y'])
	for r in range(-delta_R,delta_R+1): # 半径搜索范围是 在R=OB+-delta_R
		# 设置新半径
		R = R_0 - r
		# 与坡顶地面交点
		point_l = point_left(center,R,c['A_y'])
		# 与坡底地面交点
		if R < R_0:
			bottom_or_slope = 'slope'
		else:
			bottom_or_slope = 'bottom'
		point_r = point_right(center,R,c['A_x'],c['A_y'],c['B_x'],c['B_y'],bottom_or_slope)
		if point_r == -1:
			print('没有交点')
			break
		# 根据N 推算的分条横坐标值 维度N+1
		slice_x = slice_x_axis(point_l,point_r,c['N'])
		# 上边界的y值 维度N+1
		side_t = side_top(slice_x,c['A_x'],c['A_y'],c['B_x'],c['B_y'],c['alpha_slope'])
		# 圆弧下边界的y值 维度N+1
		side_b = side_bottom(slice_x,center,R)
		# 分条宽度
		b = slice_width(point_l,point_r,c['N'])
		# 分条中点与圆心点的夹角 维度N
		theta = slice_theta(slice_x,center,side_b)
		# 分条的重度 分条面积*gamma 维度N
		W = slice_W(slice_x,side_t,side_b,gamma)
		# 求和W_i*sin(theta_i) Fs的分母
		sum_W_theta = 0
		for i in range(len(W)):
			sum_W_theta += W[i]*math.sin(theta[i])
		# 迭代Fs至与上次之差小于0.1 
		Fs = 1
		Fs_last = 0
		# 初始设置Fs = 1
		while abs(Fs-Fs_last) > 0.1:
			sum_Fs_numerator = 0
			for i in range(len(theta)):
				m_theta = math.cos(theta[i])+math.tan(c['phi_effective'])*math.sin(theta[i])/Fs
				sum_Fs_numerator += (1/m_theta)*(c_effective*b+W[i]*math.tan(c['phi_effective']))			
			Fs_last = Fs
			Fs = round(sum_Fs_numerator/sum_W_theta,2)			
		print(str(calc_num)+'.','center:',center,'R:',R,'Fs:',Fs)
		calc_num += 1
		if Fs < Fs_min:
			Fs_min = Fs
			center_min = center
			slice_min = slice_x
			radius_min = R
		if Fs < Fs_min_each: # 为了统计各点Fs 画等势线
			Fs_min_each = Fs
	Fs_min_list.append(Fs_min_each)

print('Fanal_min','center_min:',center_min,'R:',radius_min,'Fs_min:',Fs_min)
# draw_slope(c,center_min,slice_min,radius_min)
# draw_equipotential_line(center_list,Fs_min_list)