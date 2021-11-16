# M-arm
20Fall-21Fall Srtp, SEU: M-arm with handreg&amp;pos tracing
May can use in remote medical help
# Based on JiBot(with arduino) from taobao:众灵科技(a M-arm manufactor)

# team member: 
Ruoyao Wen, Pengxiao Wu, Yihao Wu, Zheng Yi.

# Finished
Implenmented hand-reg based on Leapmotion(Unity), pos tracing based on HTC Vive tracker

# What have I done in this project?
1. Implemented Leapmotion hand-reg using Unity;
3. Finetuned pos-tracking(initial pos setting, scale transformation, smoothing, etc);
4. Implemented M-bot::arduino: process the info passed from unity, send the orders to the servos;
5. achieved Bluetooth between computer(Unity running handreg&pos_tracking) and arduino's softwareSerial(Using bluetooth module: HC-08).
